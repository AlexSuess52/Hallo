import { QuizQuestion } from '../model/quizQuestion';

export interface QuestionMessage {
  question: QuizQuestion;
  quizName: string;
}
