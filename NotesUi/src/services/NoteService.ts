import axios from 'axios'
import type { Note } from '@/types/Note'

const api = axios.create({
    baseURL: 'http://localhost:5135/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

export default {
    async getAllNotes(): Promise<Note[]> {
        try {
            const response = await api.get('/notes')
            return response.data
        } catch (error) {
            console.error('Error fetching notes:', error)
            throw error
        }
    },

    async getNote(id: number): Promise<Note> {
        try {
            const response = await api.get(`/notes/${id}`)
            return response.data
        } catch (error) {
            console.error(`Error fetching note ${id}:`, error)
            throw error
        }
    },

    async createNote(note: { title: string, text: string }): Promise<Note> {
        try {
            const response = await api.post('/notes', note)
            return response.data
        } catch (error) {
            console.error('Error creating note:', error)
            throw error
        }
    },

    async updateNote(id: number, note: { title?: string, text?: string }): Promise<Note> {
        try {
            const response = await api.put(`/notes/${id}`, note)
            return response.data
        } catch (error) {
            console.error(`Error updating note ${id}:`, error)
            throw error
        }
    },

    async deleteNote(id: number): Promise<void> {
        try {
            await api.delete(`/notes/${id}`)
        } catch (error) {
            console.error(`Error deleting note ${id}:`, error)
            throw error
        }
    }
}