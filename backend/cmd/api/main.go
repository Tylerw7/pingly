package main

import (
	"net/http"

	"log"

	config "backend/internal/config"
	database "backend/internal/database"

	"github.com/gin-gonic/gin"
)

func main() {
	var url *config.Config = config.Load()

	db, err := database.Connect(url.DB_URL)
	if err != nil {
		log.Fatal(err)
	}
	_ = db

	r := gin.Default()

	r.GET("/", func(c *gin.Context) {
		c.JSON(http.StatusOK, gin.H{
			"message": "Gin API is running",
		})
	})

	r.Run(":8080")
}
