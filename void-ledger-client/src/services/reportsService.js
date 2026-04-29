import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";

// Reports service builds query URLs and returns parsed report data for the Reports page.
export async function getReportTotals(){
    const response = await fetch(buildApiUrl("/reports/totals"));
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching reports totals failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse = await response.json();
    return parsedResponse;
}

export async function getRecentActions(take){
    const encodedTake = encodeURIComponent(take);
    const uri = buildApiUrl(`/actions/recent?take=${encodedTake}`);
    const response = await fetch(uri);
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching recent actions failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse = await response.json();
    return parsedResponse;
}

export async function getActionsByType(type, take){
    const encodedType = encodeURIComponent(type);
    const encodedTake = encodeURIComponent(take);
    const uri = buildApiUrl(`/reports/actions/by-type?type=${encodedType}&take=${encodedTake}`);
    const response = await fetch(uri);
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching actions by type failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse = await response.json();
    return parsedResponse;
}