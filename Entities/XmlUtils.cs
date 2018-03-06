using DTO;
using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Entities
{
    public class XmlUtils
    {
        static private RSN GetRSN(ReactionRSN rsn)
        {
            return new RSN
            {
                TYPE = !String.IsNullOrEmpty(rsn.CVT) ? rsn.CVT : "FREE",
                Text = !String.IsNullOrEmpty(rsn.FreeText) ? new string[] { rsn.FreeText } : null
            };
        }

        static public string GenerateXML(TanData tanData)
        {
            RXNFILE rxnFile = new RXNFILE();
            rxnFile.SOURCE = "GVKbio";
            rxnFile.VERSION = 2.ToString();
            var documents = new List<DOCUMENT>();
            LoadMasterData(tanData);
            RXNGRP rxnGroup = new RXNGRP();
            List<RXN> rxns = new List<RXN>();
            foreach (var reaction in tanData.Tan.Reactions)
            {
                #region RXNID
                var keyProduct = tanData.Tan.Participants.Where(p => p.ReactionId == reaction.Id && p.KeyProduct == true).FirstOrDefault();
                var rxnId = new RXNID();
                rxnId.RXNNUM = keyProduct?.Participant?.NUM.ToString();
                rxnId.RXNSEQ = keyProduct?.KeyProductSeq.ToString();
                #endregion

                #region RXNPROCESS
                var rxnProcess = new RXNPROCESS();
                var reactionLevelRsns = new List<RSN>();
                var xmlStages = new List<STAGE>();

                #region Reaction level RSNs
                foreach (var rsn in reaction.Tan.RSNs.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage == null && !rsn.IsIgnorableInDelivery))
                    reactionLevelRsns.Add(GetRSN(rsn));
                rxnProcess.RSN = reactionLevelRsns.ToArray();
                #endregion

                #region Stages
                foreach (var stage in reaction.Stages)
                {
                    var xmlStage = new STAGE();
                    #region Conditions (Sub stages)
                    var subStages = new List<SUBSTAGE>();
                    foreach (var condition in stage.StageConditions)
                    {
                        var subStage = new SUBSTAGE();
                        var conditions = new List<COND>();

                        if (!String.IsNullOrEmpty(condition.Temperature))
                            conditions.Add(new COND { TYPESpecified = true, TYPE = condType.TP, Text = new string[] { condition.Temperature } });
                        if (!String.IsNullOrEmpty(condition.Pressure))
                            conditions.Add(new COND { TYPESpecified = true, TYPE = condType.PR, Text = new string[] { condition.Pressure } });
                        if (!String.IsNullOrEmpty(condition.Time))
                            conditions.Add(new COND { TYPESpecified = true, TYPE = condType.TM, Text = new string[] { condition.Time } });
                        if (!String.IsNullOrEmpty(condition.PH))
                            conditions.Add(new COND { TYPESpecified = true, TYPE = condType.PH, Text = new string[] { condition.PH } });

                        subStage.COND = conditions.ToArray();
                        subStages.Add(subStage);
                    }
                    xmlStage.SUBSTAGE = subStages.ToArray();
                    #endregion

                    #region Stage level RSNs
                    var stageLevelRsns = new List<RSN>();
                    foreach (var rsn in reaction.Tan.RSNs.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.Stage.Id == stage.Id && !rsn.IsIgnorableInDelivery))
                        stageLevelRsns.Add(GetRSN(rsn));
                    xmlStage.RSN = stageLevelRsns.ToArray();
                    #endregion

                    xmlStages.Add(xmlStage);
                }
                rxnProcess.STAGE = xmlStages.ToArray();
                #endregion
                #endregion

                #region XREFGRP
                var xRefGroup = new XREFGRP();
                var nrns = new List<XREFGRPNRN>();
                foreach (var tanParticipant in tanData.Tan.Participants.Where(p => p.Reaction.Id == reaction.Id))
                {
                    XREFGRPNRN nrn;
                    if (!xRefGroup.NRN.Select(n => n.NRNNUM).Contains(tanParticipant.Participant.NUM.ToString()))
                        nrn = new XREFGRPNRN()
                        {
                            NRNNUM = tanParticipant.Participant.NUM.ToString(),
                            NRNREG = tanParticipant.Participant.RegNumber
                        };
                }
                xRefGroup.NRN = nrns.ToArray();
                #endregion

                SUBDESC subDesc = null;
                var s8000Participants = reaction.Tan.Participants.
                    Where(rp => rp.ReactionId == reaction.Id && rp.Participant.ChemicalType == ChemicalType.S8000);
                if (s8000Participants.Any())
                {
                    var subDefinitions = new List<SUBDESCSUBDEFN>();
                    foreach (var s8000Particiapnt in s8000Participants)
                    {
                        var subDefinition = new SUBDESCSUBDEFN();
                        subDefinition.NRNNUM = s8000Particiapnt.Participant.NUM.ToString();
                        subDefinition.SUBNAME = s8000Particiapnt.Participant.Name;
                        subDefinition.SUBLOC = String.Join(",", s8000Particiapnt.Participant.MetaData.Select(md => md.PageNo).ToList());
                        subDefinitions.Add(subDefinition);
                    }
                    subDesc = new SUBDESC();
                    subDesc.SUBDEFN = subDefinitions.ToArray();
                }

                #region RXN
                var rxn = new RXN();
                rxn.NO = reaction.DisplayOrder;
                rxn.NOSpecified = true;
                rxn.RXNID = rxnId;
                rxn.RSD = reaction.RSD;
                rxn.XREFGRP = xRefGroup;
                rxn.RXNPROCESS = rxnProcess;
                rxn.SUBDESC = subDesc;
                rxns.Add(rxn);
                #endregion
            }
            rxnGroup.RXN = rxns.ToArray();

            var document = new DOCUMENT();
            document.VIEW = viewType.RXN;
            document.CAN = tanData.Tan.CAN;
            document.TAN = tanData.Tan.tanNumber;
            document.ANALYST = 8005.ToString();
            document.COMMENTS = tanData.Tan.CommentsForXml;
            document.RXNGRP = rxnGroup;
            documents.Add(document);
            rxnFile.DOCUMENT = documents.ToArray();
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            var serializer = new XmlSerializer(typeof(RXNFILE), String.Empty);
            var settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true
            };
            var xmlBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(xmlBuilder, settings))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "CAS_React_Schema.xsd");
                serializer.Serialize(xmlWriter, rxnFile, ns);
            }
            return xmlBuilder.ToString();
        }

        public static void LoadMasterData(TanData tanData)
        {
            var tan = JsonConvert.DeserializeObject<Tan>(tanData.Data);
            foreach (var reaction in tan.Reactions)
            {
                foreach (var stage in reaction.Stages)
                {
                    stage.Reaction = reaction;

                    foreach (var stageCondition in stage.StageConditions)
                        stageCondition.Stage = stage;
                }
                reaction.Tan = tan;
            }
            foreach (var participant in tan.Participants)
            {
                participant.Reaction = tan.Reactions.Where(r => r.Id == participant.ReactionId).FirstOrDefault();
                participant.Stage = participant.Reaction.Stages.Where(s => s.Id == participant.StageId).FirstOrDefault();
                participant.Participant = tan.TanChemicals.Where(tc => tc.Id == participant.ParticipantId).First();
                participant.Tan = tan;
            }
            foreach (var rsn in tan.RSNs)
            {
                rsn.Reaction = tan.Reactions.Where(r => r.Id == rsn.ReactionId).FirstOrDefault();
                rsn.Stage = rsn.Reaction!=null ? rsn.Reaction.Stages.Where(s => s.Id == rsn.StageId).FirstOrDefault() : null;
                rsn.Tan = tan;
            }
            tanData.Tan = tan;
        }
    }
}