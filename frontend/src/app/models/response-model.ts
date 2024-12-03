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
    departureAirportName: string;
    departureCity: string;
    departureCountry: string;
    arrivalAirportName: string;
    arrivalCity: string;
    arrivalCountry: string;
    aiJustification: string;
}