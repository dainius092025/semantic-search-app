export function normalizeScore(score) {
  if (typeof score !== "number") {
    return null;
  }

  return score <= 1 ? score * 100 : score;
}

export function scoreLabel(scorePercent) {
  if (typeof scorePercent !== "number") {
    return null;
  }

  if (scorePercent >= 65) {
    return "Strong match";
  }

  if (scorePercent >= 45) {
    return "Good match";
  }

  if (scorePercent >= 30) {
    return "Partial match";
  }

  return "Weak match";
}

