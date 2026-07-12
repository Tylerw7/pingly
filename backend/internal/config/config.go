package config

import (
	"os"

	"github.com/joho/godotenv"
)

type Config struct {
	Port   string
	DB_URL string
}

func Load() *Config {
	godotenv.Load()

	return &Config{
		Port:   os.Getenv("PORT"),
		DB_URL: os.Getenv("DB_URL"),
	}
}
