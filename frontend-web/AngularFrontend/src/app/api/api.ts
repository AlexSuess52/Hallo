export * from './restController.service';
import { RestControllerService } from './restController.service';
export * from './triviaController.service';
import { TriviaControllerService } from './triviaController.service';
export const APIS = [RestControllerService, TriviaControllerService];
