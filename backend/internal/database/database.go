package database

import (
	// config "backend/internal/config"
	"fmt"

	// model "backend/internal/models"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

func Connect(url string) (*gorm.DB, error) {

	db, err := gorm.Open(
		postgres.Open(url), // database url
		&gorm.Config{},
	)

	if err != nil {
		return nil, fmt.Errorf("failed to get sql database: %w", err)
	}

	sqlDB, err := db.DB()
	if err != nil {
		return nil, fmt.Errorf("failed to get sql database: %w", err)
	}

	if err := sqlDB.Ping(); err != nil {
		return nil, fmt.Errorf("failed to ping database: %w", err)
	}

	fmt.Printf("Connected to Neon Database")

	return db, nil
}
