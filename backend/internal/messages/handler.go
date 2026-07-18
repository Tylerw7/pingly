package messages

import (
	"net/http"
	"regexp"

	hub "backend/internal/hub"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
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
	}
}
