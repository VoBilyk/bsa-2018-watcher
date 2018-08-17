﻿
import { ChartType } from './chart-type.enum';

export interface Chart  {
   id: number;
   type: ChartType;
   source: string;
   showCommon: string;
   threshold: number;
   mostLoaded: string;
}



