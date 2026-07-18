package health

import (
	"net/http"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

func HealthCheck(db *gorm.DB) gin.HandlerFunc {
	return func(c *gin.Context) {
		status := gin.H{"status": "ok", "db": "ok"}
		code := http.StatusOK

		sqlDB, err := db.DB()
		if err != nil || sqlDB.Ping() != nil {
			status["db"] = "unreachable"
			code = http.StatusServiceUnavailable
		}

		c.JSON(code, status)
	}
}
