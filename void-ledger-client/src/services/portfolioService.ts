import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";
import type { PortfolioValuationDto } from "./apiTypes";

// Portfolio service returns parsed valuation data or throws a display-ready error.
async function getPortfolioValuation() : Promise<PortfolioValuationDto> {

    const response = await fetch(buildApiUrl("/portfolio/valuation"));
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching portfolio failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse : PortfolioValuationDto = await response.json();
    return parsedResponse;

}

export default getPortfolioValuation