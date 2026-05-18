import axios from 'axios'

export const api = axios.create({
  baseURL: typeof window !== 'undefined' ? window.location.origin : import.meta.env.VITE_API_URL,
  timeout: 15000,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})
