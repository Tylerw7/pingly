package database

import (
	// config "backend/internal/config"
	"backend/internal/models"
	"log"
	"time"

	// model "backend/internal/models"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
)

func Connect(url string) *gorm.DB {

	db, err := gorm.Open(postgres.Open(url), &gorm.Config{
		Logger: logger.Default.LogMode(logger.Warn),
	},
	)

	if err != nil {
		log.Fatalf("Failed to connect to database: %v", err)
	}

	sqlDB, err := db.DB()
	if err != nil {
		log.Fatalf("failed to get sql.DB: %v", err)
	}

	sqlDB.SetMaxOpenConns(10)
	sqlDB.SetMaxIdleConns(2)
	sqlDB.SetConnMaxIdleTime(5 * time.Minute)

	//AutoMigrate for now
	if err := db.AutoMigrate(
		&models.User{},
		&models.Topic{},
		&models.Message{},
	); err != nil {
		log.Fatalf("failed to migrate: %v", err)
	}

	log.Println("database connected and migrated")
	return db
}
