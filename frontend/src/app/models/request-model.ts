export interface FlightPlanRequest {
    id: number;
    departureICAO: string;
    arrivalICAO: string;
    departureTime: string;
    flightDay: string;
    flightDuration: string;
    aircraftId: number;
    userId: string;
}