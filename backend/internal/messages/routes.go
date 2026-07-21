package messages

import (
	hub "backend/internal/hub"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

func RegisterRoutes(r *gin.Engine, db *gorm.DB) {

	api := r.Group("/api")
	{
		api.POST("/topics/:topic", Publish(db, &hub.Hub{}))
		api.GET("/topics/:topic/messages", History(db))
	}

}
