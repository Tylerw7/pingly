package subscribe

import (
	hub "backend/internal/hub"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

func RegisterRoutes(r *gin.Engine, db *gorm.DB) {

	r.GET("/topics/:topic/sse", Subscribe(&hub.Hub{}))

}
