export interface FlightPlanResponse {
    responseId: number;
    departureICAO: string;
    arrivalICAO: string;
    departureTime: string;
    flightDay: string;
    flightDuration: string;
    aircraftId: number;
    departureMETAR: string;
    arrivalMETAR: string;
    departureTAF: string;
    arrivalTAF: string;
}