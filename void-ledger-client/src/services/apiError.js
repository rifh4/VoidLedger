// Prefer backend problem-details text when available, then fall back to a generic service message.
export async function getErrorMessage(response, fallbackMessage) {
  try {
    const errorBody = await response.json();

    if (errorBody.detail) {
      return errorBody.detail;
    }

    if (errorBody.title) {
      return errorBody.title;
    }

    return fallbackMessage;
  }
  catch {
    return fallbackMessage;
  }
}