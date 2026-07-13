package main

import (
	"net/http"

	"log"

	"github.com/gin-contrib/cors"

	config "backend/internal/config"
	database "backend/internal/database"
	"backend/internal/health"

	"github.com/gin-gonic/gin"
)

func main() {
	var url *config.Config = config.Load()

	db, err := database.Connect(url.DatabaseURL)
	if err != nil {
		log.Fatal(err)
	}
	_ = db

	r := gin.Default()

	r.Use(cors.Default())

	r.GET("/", func(c *gin.Context) {
		c.JSON(http.StatusOK, gin.H{
			"message": "Gin API is running",
		})
	})

	health.RegisterRoutes(r)

	r.Run(":8080")
}
