/**
 * Discipline types for workout categorization
 */

export enum Discipline {
  HYROX = 'HYROX',
  RUNNING = 'RUNNING',
  STRENGTH = 'STRENGTH',
  HYBRID = 'HYBRID',
  REST = 'REST',
}

export interface DisciplineColors {
  primary: string;
  secondary: string;
  bg: string;
  text: string;
  border: string;
}

export interface DisciplineConfig {
  name: string;
  colors: DisciplineColors;
  icon: string;
  description: string;
}
