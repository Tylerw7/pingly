package models

import "time"

type Topic struct {
	ID        uint      `gorm:"primaryKey" json:"id"`
	Name      string    `gorm:"uniqueIndex;size:64;not null" json:"name"`
	OwnerID   string    `gorm:"index" json:"-"`
	CreatedAt time.Time `json:"created_at"`

	Messages []Message `gorm:"constraint:OnDelete:CASCADE" json:"-"`
}
