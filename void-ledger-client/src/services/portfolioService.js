import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";

// Portfolio service returns parsed valuation data or throws a display-ready error.
async function getPortfolioValuation(){

    const response = await fetch(buildApiUrl("/portfolio/valuation"));
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching portfolio failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse = await response.json();
    return parsedResponse;

}

export default getPortfolioValuation