export default function ErrorMessage({ error }: { error?: unknown }) {
  if (!error) return null
  const text = error instanceof Error ? error.message : String(error)
  return <div style={{ color: '#b00020' }}>Error: {text}</div>
}
