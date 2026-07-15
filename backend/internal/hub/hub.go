package hub

import "sync"

type Hub struct {
	mu     sync.RWMutex
	topics map[string]map[chan []byte]struct{}
}

func New() *Hub {
	return &Hub{
		topics: make(map[string]map[chan []byte]struct{}),
	}
}
