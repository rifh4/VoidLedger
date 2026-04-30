// Local development uses the Vite '/api' proxy, production can override this with VITE_API_BASE_URL.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "/api";

// Build consistent API URLs without duplicating base-url logic across service files.
export function buildApiUrl(path: string): string {
    const normalizedBaseUrl = API_BASE_URL.endsWith("/")
        ? API_BASE_URL.slice(0, -1)
        : API_BASE_URL;

    const normalizedPath = path.startsWith("/")
        ? path
        : `/${path}`;

    return `${normalizedBaseUrl}${normalizedPath}`;
}