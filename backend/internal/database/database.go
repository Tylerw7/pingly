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

func Connect(url string) (*gorm.DB, error) {

	db, err := gorm.Open(postgres.Open(url), &gorm.Config{
		Logger: logger.Default.LogMode(logger.Warn),
	})

	if err != nil {
		return nil, err
	}

	sqlDB, err := db.DB()
	if err != nil {
		return nil, err
	}

	sqlDB.SetMaxOpenConns(10)
	sqlDB.SetMaxIdleConns(2)
	sqlDB.SetConnMaxIdleTime(5 * time.Minute)

	if err := db.AutoMigrate(
		&models.User{},
		&models.Topic{},
		&models.Message{},
	); err != nil {
		return nil, err
	}

	log.Println("Database connected and migrated")

	return db, nil
}
