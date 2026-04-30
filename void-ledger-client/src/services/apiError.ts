type ApiErrorBody = {
  detail?: string;
  title?: string;
};

// Prefer backend problem-details text when available, then fall back to a generic service message.
export async function getErrorMessage(
    response: Response,
    fallbackMessage: string
  ): Promise<string> {
    try {
      const errorBody: ApiErrorBody = await response.json();
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