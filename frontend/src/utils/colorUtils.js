/**
 * Egy hex színt HSL komponensekre bont.
 * @returns {{ h: number, s: number, l: number }} — h: 0-360, s/l: 0-100
 */
function hexToHsl(hex) {
  const r = parseInt(hex.slice(1, 3), 16) / 255
  const g = parseInt(hex.slice(3, 5), 16) / 255
  const b = parseInt(hex.slice(5, 7), 16) / 255
  const max = Math.max(r, g, b), min = Math.min(r, g, b)
  let h = 0, s = 0
  const l = (max + min) / 2

  if (max !== min) {
    const d = max - min
    s = l > 0.5 ? d / (2 - max - min) : d / (max + min)
    switch (max) {
      case r: h = ((g - b) / d + (g < b ? 6 : 0)) / 6; break
      case g: h = ((b - r) / d + 2) / 6; break
      case b: h = ((r - g) / d + 4) / 6; break
    }
  }

  return { h: Math.round(h * 360), s: Math.round(s * 100), l: Math.round(l * 100) }
}

/**
 * Pasztell hex → sötét, de jól látható, SZÍNES háttér dark módhoz.
 * Ikonhátterekhez: megtartja a tónust, csökkenti a fényességet ~22%-ra.
 */
export function darkenForDark(hex) {
  if (!hex || !hex.startsWith('#') || hex.length < 7) return null
  const { h } = hexToHsl(hex)
  return `hsl(${h}, 58%, 22%)`
}

/**
 * Pasztell hex → enyhén tintált sötét lap-háttér dark módhoz.
 * Kártyáknál: szinte ugyanolyan sötét, de minimális tónusra emlékeztet.
 */
export function tintForDark(hex) {
  if (!hex || !hex.startsWith('#') || hex.length < 7) return null
  const { h } = hexToHsl(hex)
  return `hsl(${h}, 28%, 17%)`
}

/**
 * Pasztell hex → élénkebb, telítettebb szín dark módhoz.
 * Dekorációs sávokhoz (MainTitle), ahol jobb a kontrasztosabb szín.
 */
export function vividForDark(hex) {
  if (!hex || !hex.startsWith('#') || hex.length < 7) return null
  const { h } = hexToHsl(hex)
  return `hsl(${h}, 72%, 62%)`
}
