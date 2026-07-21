package router

import (
	"github.com/gin-gonic/gin"
	"gorm.io/gorm"

	health "backend/internal/health"
	messages "backend/internal/messages"
)

func SetUpRouter(r *gin.Engine, db *gorm.DB) {

	// register routes
	health.RegisterRoutes(r, db)

	messages.RegisterRoutes(r, db)

}
