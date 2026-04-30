import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";
import type { MessageResponseDto } from "./apiTypes";

// Trading service keeps deposit, buy, and sell request shapes out of the page component.
export async function deposit(amount: number): Promise<MessageResponseDto> {
    const response = await fetch(buildApiUrl("/deposit"), 
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                amount
            })
        }
    );
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Deposit failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: MessageResponseDto = await response.json();
    return parsedResponse;
}

export async function buyCommodity(
    name: string,
    qty: number
): Promise<MessageResponseDto> {
    const response = await fetch(buildApiUrl("/trade/buy"), 
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                name, 
                qty
            })
        }
    );
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Buy failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: MessageResponseDto = await response.json();
    return parsedResponse;
}

export async function sellCommodity(
    name: string,
    qty: number
): Promise<MessageResponseDto> {
    const response = await fetch(buildApiUrl("/trade/sell"), 
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                name, 
                qty
            })
        }
    );
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Sell failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: MessageResponseDto = await response.json();
    return parsedResponse;
}