package subscribe

import (
	"backend/internal/hub"
	"io"
	"net/http"
	"regexp"
	"time"

	"github.com/gin-gonic/gin"
)

var topicNameRe = regexp.MustCompile(`^[a-zA-Z0-9_-]{1,64}$`)

// Subscribe handles GET /api/topics/:topic/sse
// Opens a Server-Sent Events stream. The connection stays open; each new
// message on the topic is pushed as an SSE `message` event. A keepalive
// comment is sent every 30s so proxies don't kill the idle connection.
//
// Test it:  curl -N localhost:8080/api/topics/mytopic/sse

func Subscribe(h *hub.Hub) gin.HandlerFunc {
	return func(c *gin.Context) {
		topicName := c.Param("topic")
		if !topicNameRe.MatchString(topicName) {
			c.JSON(http.StatusBadRequest, gin.H{"error": "invalid topic name"})
			return
		}

		c.Header("Content-Type", "text/event-stream")
		c.Header("Cache-Control", "no-cache")
		c.Header("Connection", "keep-alive")

		// Tell the client we're open (mirrors ntfy's "open" event).
		c.SSEvent("open", gin.H{"topic": topicName})
		c.Writer.Flush()

		ch, unsubscribe := h.Subscribe(topicName)
		defer unsubscribe()

		keepalive := time.NewTicker(30 * time.Second)
		defer keepalive.Stop()

		// c.Stream keeps calling this function; returning false ends the stream.
		c.Stream(func(w io.Writer) bool {
			select {
			case <-c.Request.Context().Done():
				return false // client disconnected
			case payload := <-ch:
				// payload is already JSON bytes; send raw so it isn't double-encoded.
				c.Render(-1, sseEvent{event: "message", data: payload})
				return true
			case <-keepalive.C:
				w.Write([]byte(": keepalive\n\n"))
				return true
			}
		})
	}
}

// sseEvent renders a raw pre-marshaled JSON payload as an SSE event.
// Gin's built-in c.SSEvent would JSON-encode the payload a second time
// (turning it into an escaped string), so we write the frame ourselves.
type sseEvent struct {
	event string
	data  []byte
}

func (e sseEvent) Render(w http.ResponseWriter) error {
	_, err := w.Write([]byte("event: " + e.event + "\ndata: " + string(e.data) + "\n\n"))
	return err
}

func (e sseEvent) WriteContentType(w http.ResponseWriter) {
	w.Header().Set("Content-Type", "text/event-stream")
}
