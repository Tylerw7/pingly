package messages

import (
	"encoding/json"
	"io"
	"net/http"
	"regexp"
	"strconv"

	hub "backend/internal/hub"
	"backend/internal/models"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
	"gorm.io/gorm/clause"
)

var topicNameRe = regexp.MustCompile(`^[a-zA-Z0-9_-]{1,64}$`)

type publishRequest struct {
	Title    string `json:"title"`
	Body     string `json:"body"`
	Priority int    `json:"priority"`
}

func Publish(db *gorm.DB, h *hub.Hub) gin.HandlerFunc {
	return func(c *gin.Context) {
		topicName := c.Param("topic")
		if !topicNameRe.MatchString(topicName) {
			c.JSON(http.StatusBadRequest, gin.H{"error": "invalid topic name"})
			return
		}

		// Parse body content - JSON if content type says so, otherwise plan text.
		var req publishRequest
		if c.ContentType() == "application/json" {
			if err := c.ShouldBindJSON(&req); err != nil {
				c.JSON(http.StatusBadRequest, gin.H{"error": "invalid JSON body"})
				return
			}
		} else {
			raw, err := io.ReadAll(io.LimitReader(c.Request.Body, 8192))
			if err != nil {
				c.JSON(http.StatusBadRequest, gin.H{"error": "failed to read body"})
				return
			}
			req.Body = string(raw)
			// ntfy-style header shortcuts for plain-text publishes
			req.Title = c.GetHeader("X-Title")
			if p := c.GetHeader("X-Priority"); p != "" {
				req.Priority, _ = strconv.Atoi(p)
			}

		}

		if req.Body == "" {
			c.JSON(http.StatusBadRequest, gin.H{"Error": "message body is required."})
		}

		if req.Priority < 1 || req.Priority > 5 {
			req.Priority = 3
		}

		// Find or create the topic (topics spring into existence on publish).
		topic := models.Topic{Name: topicName}
		if err := db.Clauses(clause.OnConflict{DoNothing: true}).Create(&topic).Error; err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to upsert topic"})
			return
		}
		if topic.ID == 0 { // Already existed; fetch it
			if err := db.Where("name = ?", topicName).First(&topic).Error; err != nil {
				c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to load data for topic"})
				return
			}
		}

		msg := models.Message{
			TopicID:  topic.ID,
			Title:    req.Title,
			Body:     req.Body,
			Priority: req.Priority,
		}

		if err := db.Create(&msg).Error; err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "failed to save message"})
			return
		}
		msg.Topic = topicName

		// Fan out to live subscribers. Marshal once, reuse for everyone.
		payload, err := json.Marshal(msg)
		if err == nil {
			h.Publish(topicName, payload)
		}

		c.JSON(http.StatusCreated, msg)

	}
}

// History handles GET /api/topics/:topic/messages — recent messages,
// newest first. Useful for the web/mobile apps to backfill on load.
func History(db *gorm.DB) gin.HandlerFunc {
	return func(c *gin.Context) {
		topicName := c.Param("topic")

		var topic models.Topic
		if err := db.Where("name = ?", topicName).First(&topic).Error; err != nil {
			// Unknown topic = empty history, not an error (topics are lazy).
			c.JSON(http.StatusOK, []models.Message{})
			return
		}

		limit := 50
		if l, err := strconv.Atoi(c.Query("limit")); err == nil && l > 0 && l <= 200 {
			limit = l
		}

		var messages []models.Message
		db.Where("topic_id = ?", topic.ID).
			Order("created_at DESC").
			Limit(limit).
			Find(&messages)

		for i := range messages {
			messages[i].Topic = topicName
		}

		c.JSON(http.StatusOK, messages)
	}
}
