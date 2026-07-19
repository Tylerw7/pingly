package hub

import "sync"

type Hub struct {
	mu     sync.RWMutex
	topics map[string]map[chan []byte]struct{}
}

func (h *Hub) Publish(topicName string, payload []byte) {
	panic("unimplemented")
}

func New() *Hub {
	return &Hub{
		topics: make(map[string]map[chan []byte]struct{}),
	}
}

func (h *Hub) Subscribe(topic string) (ch chan []byte, unsubscribe func()) {

	ch = make(chan []byte, 16)

	h.mu.Lock()
	if h.topics[topic] == nil {
		h.topics[topic] = make(map[chan []byte]struct{})
	}
	h.topics[topic][ch] = struct{}{}
	h.mu.Unlock()

	unsubscribe = func() {
		h.mu.Lock()
		delete(h.topics[topic], ch)
		if len(h.topics[topic]) == 0 {
			delete(h.topics, topic)
		}
		h.mu.Unlock()
	}
	return ch, unsubscribe
}
