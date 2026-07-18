package main

import (
	"log"
	"net/http"

	"github.com/gin-contrib/cors"

	config "backend/internal/config"
	database "backend/internal/database"
	router "backend/internal/router"

	"github.com/gin-gonic/gin"
)

func main() {
	var url *config.Config = config.Load()

	r := gin.Default()

	r.Use(cors.Default())

	db, err := database.Connect(url.DatabaseURL)

	if err != nil {
		log.Fatal(err)
	}

	r.GET("/", func(c *gin.Context) {
		c.JSON(http.StatusOK, gin.H{
			"message": "Gin API is running",
		})
	})

	router.SetUpRouter(r, db)

	r.Run(":8080")
}
