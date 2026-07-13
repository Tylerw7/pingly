package config

import (
	"log"
	"os"

	"github.com/joho/godotenv"
)

type Config struct {
	Port        string
	DatabaseURL string
}

func Load() *Config {
	_ = godotenv.Load()

	cfg := &Config{
		Port:        os.Getenv("PORT"),
		DatabaseURL: os.Getenv("DB_URL"),
	}

	if cfg.DatabaseURL == "" {
		log.Fatal("DATABASE_URL is required")
	}

	return cfg
}
