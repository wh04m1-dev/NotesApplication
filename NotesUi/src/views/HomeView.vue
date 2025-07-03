<script setup lang="ts">
import { ref, onMounted } from 'vue'
import NoteService from '@/services/NoteService'
import type { Note } from '@/types/Note'

// Reactive state
const notes = ref<Note[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const showModal = ref(false)
const currentNote = ref<Partial<Note>>({ title: '', text: '' })
const isEditing = ref(false)

// Fetch all notes
const fetchNotes = async () => {
  try {
    loading.value = true
    notes.value = await NoteService.getAllNotes()
    error.value = null
  } catch (err) {
    error.value = 'Failed to load notes. Please try again later.'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Handle form submission
const handleSubmit = async () => {
  try {
    if (isEditing.value && currentNote.value.id) {
      await NoteService.updateNote(currentNote.value.id, {
        title: currentNote.value.title,
        text: currentNote.value.text
      })
    } else {
      await NoteService.createNote({
        title: currentNote.value.title || '',
        text: currentNote.value.text || ''
      })
    }
    showModal.value = false
    await fetchNotes()
  } catch (err) {
    error.value = isEditing.value
      ? 'Failed to update note'
      : 'Failed to create note'
    console.error(err)
  }
}

// Edit a note
const editNote = (note: Note) => {
  currentNote.value = { ...note }
  isEditing.value = true
  showModal.value = true
}

// Delete a note
const deleteNote = async (id: number) => {
  if (confirm('Are you sure you want to delete this note?')) {
    try {
      await NoteService.deleteNote(id)
      await fetchNotes()
    } catch (err) {
      error.value = 'Failed to delete note'
      console.error(err)
    }
  }
}

// Format date for display
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}

// Initialize component
onMounted(fetchNotes)
</script>

<template>
  <main class="container mx-auto p-4 max-w-6xl">
    <!-- Header and Create Button -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Notes App</h1>
      <button @click="() => {
        currentNote = { title: '', text: '' }
        isEditing = false
        showModal = true
      }" class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">
        Create Note
      </button>
    </div>

    <!-- Error Message -->
    <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
      {{ error }}
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-8">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-600"></div>
      <p class="mt-2 text-gray-600">Loading notes...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="notes.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
      <p class="text-gray-500">No notes found. Create your first note!</p>
    </div>

    <!-- Notes Table -->
    <div v-else class="overflow-x-auto bg-white rounded-lg shadow">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Title</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Content</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Updated</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="note in notes" :key="note.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ note.id }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ note.title }}</td>
            <td class="px-6 py-4 text-sm text-gray-500 max-w-xs truncate">{{ note.text }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ formatDate(note.createdAt) }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ formatDate(note.updatedAt) }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button @click="editNote(note)" class="text-blue-600 hover:text-blue-900 mr-3">
                Edit
              </button>
              <button @click="deleteNote(note.id)" class="text-red-600 hover:text-red-900">
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Note Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-lg shadow-xl w-full max-w-md">
        <div class="p-6">
          <h2 class="text-xl font-semibold mb-4">
            {{ isEditing ? 'Edit Note' : 'Create Note' }}
          </h2>

          <form @submit.prevent="handleSubmit">
            <div class="mb-4">
              <label class="block text-gray-700 mb-2">Title</label>
              <input v-model="currentNote.title" required
                class="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500">
            </div>

            <div class="mb-6">
              <label class="block text-gray-700 mb-2">Content</label>
              <textarea v-model="currentNote.text" required rows="4"
                class="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"></textarea>
            </div>

            <div class="flex justify-end space-x-3">
              <button type="button" @click="showModal = false"
                class="px-4 py-2 border border-gray-300 rounded hover:bg-gray-50">
                Cancel
              </button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
                {{ isEditing ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </main>
</template>