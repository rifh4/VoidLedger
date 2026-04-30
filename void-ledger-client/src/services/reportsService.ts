import { getErrorMessage } from "./apiError";
import { buildApiUrl } from "./apiConfig";
import type { ActionListResponseDto, ReportTotalsDto } from "./apiTypes";

// Reports service builds query URLs and returns parsed report data for the Reports page.
export async function getReportTotals(): Promise<ReportTotalsDto> {
    const response = await fetch(buildApiUrl("/reports/totals"));
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching reports totals failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: ReportTotalsDto = await response.json();
    return parsedResponse;
}

export async function getRecentActions(take: number): Promise<ActionListResponseDto> {
    const encodedTake = encodeURIComponent(take.toString());
    const uri = buildApiUrl(`/actions/recent?take=${encodedTake}`);
    const response = await fetch(uri);
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching recent actions failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: ActionListResponseDto = await response.json();
    return parsedResponse;
}

export async function getActionsByType(
    type: string,
    take: number
): Promise<ActionListResponseDto> {
    const encodedType = encodeURIComponent(type);
    const encodedTake = encodeURIComponent(take.toString());
    const uri = buildApiUrl(`/reports/actions/by-type?type=${encodedType}&take=${encodedTake}`);
    const response = await fetch(uri);
    if(!response.ok){
        const errorMessage = await getErrorMessage(response, "Fetching actions by type failed.");
        throw new Error(errorMessage);
    }
    const parsedResponse: ActionListResponseDto = await response.json();
    return parsedResponse;
}