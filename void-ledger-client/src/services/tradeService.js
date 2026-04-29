import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";

// Trading service keeps deposit, buy, and sell request shapes out of the page component.
export async function deposit(amount){
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
    const parsedResponse = await response.json();
    return parsedResponse;
}

export async function buyCommodity(name, qty){
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
    const parsedResponse = await response.json();
    return parsedResponse;
}

export async function sellCommodity(name, qty){
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
    const parsedResponse = await response.json();
    return parsedResponse;
}