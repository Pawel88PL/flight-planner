import { AIResponseModel } from "./ai-response-model";
import { AirportModel } from "./airport-model";

export interface FlightPlanResponse {
    id: number;
    flightDay: string;
    flightDuration: string;
    departureTime: string;
    aircraftId: number;
    createdAt: string;
    departureAirport: AirportModel;
    arrivalAirport: AirportModel;
    aiJustification: AIResponseModel;
}