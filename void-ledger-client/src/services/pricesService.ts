import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";
import type { PriceDto, MessageResponseDto } from "./apiTypes";

// Price service owns list, update, and lookup API calls for the Prices page.
export async function getPrices(): Promise<PriceDto[]> {
    const response = await fetch(buildApiUrl("/prices"));
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching prices failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: PriceDto[] = await response.json();
    return parsedResponse;
}

export async function setPrice(
    name: string,
    price: number
): Promise<MessageResponseDto> {
    const response = await fetch(buildApiUrl("/prices"), 
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                name,
                price
            })
        }
    )
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Setting price failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: MessageResponseDto = await response.json();
    return parsedResponse;
}

export async function getPriceByName(name: string): Promise<PriceDto> {
    const encodedName = encodeURIComponent(name);
    const uri = buildApiUrl(`/prices/${encodedName}`);
    const response = await fetch(uri);
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Search failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: PriceDto = await response.json();
    return parsedResponse;
}

