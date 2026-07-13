package models

import "time"

type Message struct {
	ID        uint      `gorm:"primaryKey" json:"id"`
	TopicID   uint      `gorm:"index;not null" json:"topic_id"`
	Topic     string    `gorm:"-" json:"topic"`
	Title     string    `gorm:"size:256" json:"title,omitempty"`
	Body      string    `gorm:"type:text;not null" json:"body"`
	Priority  int       `gorm:"default:3" json:"priority"`
	CreatedAt time.Time `json:"created_at"`
}
