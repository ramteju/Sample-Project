﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TanDTO
    {
        public TanDTO()
        {
            Reactions = new List<TanReaction>();
            RSN = new List<RSNDTO>();
            RSD = new List<RSDDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RXNcount { get; set; }
        public List<TanReaction> Reactions { get; set; }
        public List<RSNDTO> RSN { get; set; }
        public List<RSDDTO> RSD { get; set; }

    }
    public class TanReaction
    {
        public TanReaction()
        {
            Stages = new List<ReactionStage>();
        }
        public Guid Id { get; set; }
        public List<ReactionStage> Stages { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

    }
    public class ReactionStage
    {
        public ReactionStage()
        {
            Conditions = new List<DTO.ConditionsDTO>();
        }
        public int Id { get; set; }
        public string StageName { get; set; }
        public List<ConditionsDTO> Conditions { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class RSNDTO
    {
        public Guid Id { get; set; }
        public Guid ReactionID { get; set; }
        public Guid? StageId { get; set; }
        public string CVT { get; set; }
        public string FreeText { get; set; }
        public int DisplayOrder { get; set; }
        public int TanId { get; set; }
    }


    public class RSDDTO
    {
        public Guid Id { get; set; }
        public Guid ReactionId { get; set; }
        public Guid? StageId { get; set; }
        public float Yield { get; set; }
        public Chemical Participant { get; set; }
        public ParticipantType Participanttype { get; set; }
        public int DisplayOrder { get; set; }
        public int TanId { get; set; }
    }


    public class Chemical
    {
        public Guid Id { get; set; }
        public ChemicalType ChemicalType { get; set; }
        public int NUM { get; set; }
        public string RegNumber { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
    public class ConditionsDTO
    {
        public Guid Id { get; set; }
        public string Temperature { get; set; }
        public string Pressure { get; set; }
        public string Time { get; set; }
        public string PH { get; set; }

        public string TEMP_TYPE { get; set; }
        public string TIME_TYPE { get; set; }
        public string PH_TYPE { get; set; }
        public string PRESSURE_TYPE { get; set; }

        public int DisplayOrder { get; set; }
        public Guid StageId { get; set; }
    }
}