package router

import (
	"github.com/gin-gonic/gin"

	health "backend/internal/health"
)

func SetUpRouter() *gin.Engine {
	r := gin.Default()

	// register routes
	health.RegisterRoutes(r)

	return r
}
