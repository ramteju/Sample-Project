using Client.Command;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using Client.Models;
using System.Text.RegularExpressions;
using Client.Common;
using Entities;
using Client.XML;
using Client.Logging;
using Client.ViewModel;

namespace Client.ViewModels.Core
{
    public class TanCommentsVM : PropertyChangedNotification
    {
        public TanCommentsVM(ObservableCollection<Comments> Comments)
        {
            TanComments = new ObservableCollection<Comments>(Comments);
            EditCommentCommand = new Command.DelegateCommand(this.EditComment);
            SaveAuthorComment = new DelegateCommand(this.AddAuthorComment);
            SaveOtherComment = new DelegateCommand(this.AddOtherComment);
            SaveCASComment = new DelegateCommand(this.AddCASComment);
            SaveIndexComment = new DelegateCommand(this.AddIndexComment);
            ClearAllFields = new Command.DelegateCommand(this.ClearVM);
            DeleteTanComment = new Command.DelegateCommand(this.DeleteComment);
            string pattern = S.RegularExpressions
                .Where(re => re.RegulerExpressionFor == ProductTracking.Models.Core.RegulerExpressionFor.FreeText)
                .Select(re => re.Expression).FirstOrDefault();
            CommentRegExp = new Regex(string.IsNullOrEmpty(pattern) ? "" : pattern);
            TanComments.CollectionChanged += TanComments_CollectionChanged;
            TanComments.UpdateDisplayOrder();
        }

        private void TanComments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TanComments.UpdateDisplayOrder();
        }

        private void DeleteComment(object obj)
        {

            try
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you want to delete Selected Comment?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    var Comment = TanComments.Where(tc => tc.Id == SelectedComment.Id).FirstOrDefault();
                    TanComments.Remove(Comment);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private ObservableCollection<Comments> tanComments;
        private int indexNUM;
        private string indexPAGE;
        private string indexLINE;
        private string indexPARA;
        private string indexCOLUMN;
        private string indexTABLE;
        private string indexFIGURE;
        private string indexSCHEMES;
        private string indexSHEET;
        private string indexFOOTNOTE;
        private string indexCOMMENT;

        private int authorNUM;
        private string authorPAGE;
        private string authorLINE;
        private string authorPARA;
        private string authorCOLUMN;
        private string authorTABLE;
        private string authorFIGURE;
        private string authorSCHEMES;
        private string authorSHEET;
        private string authorFOOTNOTE;
        private string authorCOMMENT;
        private bool isAuthorNumCorrected;

        private int otherNUM;
        private string otherCOMMENT;

        private string temperatureCOMMENT;
        private string defaultCOMMENT;

        private string casConsultedCOMMENT;


        private Comments selectedComment;
        Regex CommentRegExp;
        private int commentLegth;
        private int selectedTab;

        public ObservableCollection<Comments> TanComments
        {
            get { return tanComments; }
            set
            {
                SetProperty(ref tanComments, value);
                CommentLegth = (from c in TanComments where c.CommentType != CommentType.DEFAULT select c.Length).ToList().Sum();
            }
        }
        [Required(ErrorMessage = "NUM is Required")]
        public int IndexNUM
        {
            get { return indexNUM; }
            set
            {
                SetProperty(ref indexNUM, value);
            }
        }
        public string IndexPAGE
        {
            get { return indexPAGE; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexPAGE, value);
            }
        }
        public string IndexLINE
        {
            get { return indexLINE; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexLINE, value);
            }
        }
        public string IndexPARA
        {
            get { return indexPARA; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexPARA, value);
            }
        }
        public string IndexCOLUMN
        {
            get { return indexCOLUMN; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexCOLUMN, value);
            }
        }
        public string IndexTABLE
        {
            get { return indexTABLE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexTABLE, value);
            }
        }
        public string IndexFIGURE
        {
            get { return indexFIGURE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexFIGURE, value);
            }
        }
        public string IndexSCHEMES
        {
            get { return indexSCHEMES; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexSCHEMES, value);
            }
        }
        public string IndexSHEET
        {
            get { return indexSHEET; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexSHEET, value);
            }
        }
        public string IndexFOOTNOTE
        {
            get { return indexFOOTNOTE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexFOOTNOTE, value);
            }
        }
        public string IndexCOMMENT
        {
            get { return indexCOMMENT; }
            set
            {
                if (value != null && (CommentRegExp.IsMatch(value) || value == string.Empty) && !value.ToLower().Contains("comment")) { }
                else
                    MessageBox.Show("Special Characters are not allowed.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref indexCOMMENT, value);
            }
        }



        public int AuthorNUM
        {
            get { return authorNUM; }
            set
            {
                if (value != 0 && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorNUM, value);
            }
        }
        public string AuthorPAGE
        {
            get { return authorPAGE; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorPAGE, value);
            }
        }
        public string AuthorLINE
        {
            get { return authorLINE; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorLINE, value);
            }
        }
        public string AuthorPARA
        {
            get { return authorPARA; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorPARA, value);
            }
        }
        public string AuthorCOLUMN
        {
            get { return authorCOLUMN; }
            set
            {
                if (!string.IsNullOrEmpty(value) && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorCOLUMN, value);
            }
        }
        public string AuthorTABLE
        {
            get { return authorTABLE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorTABLE, value);
            }
        }
        public string AuthorFIGURE
        {
            get { return authorFIGURE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorFIGURE, value);
            }
        }
        public string AuthorSCHEMES
        {
            get { return authorSCHEMES; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorSCHEMES, value);
            }
        }
        public string AuthorSHEET
        {
            get { return authorSHEET; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorSHEET, value);
            }
        }
        public string AuthorFOOTNOTE
        {
            get { return authorFOOTNOTE; }
            set
            {
                if (value != null && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorFOOTNOTE, value);
            }
        }
        public string AuthorCOMMENT
        {
            get { return authorCOMMENT; }
            set
            {
                if (value != null && (CommentRegExp.IsMatch(value) || value == string.Empty) && !value.ToLower().Contains("comment"))
                {

                }
                else
                    MessageBox.Show("Special Characters are not allowed.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref authorCOMMENT, value);
            }
        }
        public bool IsAuthorNumCorrected { get { return isAuthorNumCorrected; } set { SetProperty(ref isAuthorNumCorrected, value); } }

        public int OtherNUM
        {
            get { return otherNUM; }
            set
            {
                if (value != 0 && S.CommentParameters.Any(x => value.ToString().ToLower().Contains(x)))
                    MessageBox.Show("Comment parameters are not allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref otherNUM, value);
            }
        }
        public string OtherCOMMENT
        {
            get { return otherCOMMENT; }
            set
            {
                if (value != null && (CommentRegExp.IsMatch(value) || value == string.Empty) && !value.ToLower().Contains("comment"))
                {

                }
                else
                    MessageBox.Show("Special Characters are not allowed.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref otherCOMMENT, value);
            }
        }
        public string TemperatureCOMMENT { get { return temperatureCOMMENT; } set { SetProperty(ref temperatureCOMMENT, value); } }
        public string DefaultCOMMENT { get { return defaultCOMMENT; } set { SetProperty(ref defaultCOMMENT, value); } }
        public string CasConsultedCOMMENT
        {
            get { return casConsultedCOMMENT; }
            set
            {
                if (value != null && (CommentRegExp.IsMatch(value) || value == string.Empty) && !value.ToLower().Contains("comment"))
                {

                }
                else
                    MessageBox.Show("Special Characters and comment parameters are not allowed.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProperty(ref casConsultedCOMMENT, value);
            }
        }

        public Comments SelectedComment { get { return selectedComment; } set { SetProperty(ref selectedComment, value); } }

        public int CommentLegth { get { return commentLegth; } set { SetProperty(ref commentLegth, value); } }
        public int SelectedTab { get { return selectedTab; } set { SetProperty(ref selectedTab, value); } }
        public Comments EditingComment;



        public DelegateCommand SaveIndexComment { get; private set; }
        public DelegateCommand SaveAuthorComment { get; private set; }
        public DelegateCommand SaveOtherComment { get; private set; }
        public DelegateCommand SaveCASComment { get; private set; }
        public DelegateCommand EditCommentCommand { get; private set; }
        public DelegateCommand ClearAllFields { get; private set; }
        public DelegateCommand DeleteTanComment { get; private set; }


        private void AddIndexComment(object obj)
        {
            try
            {
                string ErrMsg = string.Empty;
                if (ValidateIndexComments(out ErrMsg))
                {
                    string Index = ("NUM " + IndexNUM) + (!string.IsNullOrEmpty(IndexPAGE) ? ",Page " + IndexPAGE : string.Empty) + (!string.IsNullOrEmpty(IndexLINE) ? ",Line " + IndexLINE : string.Empty) + (!string.IsNullOrEmpty(IndexPARA) ? ",Para " + IndexPARA : string.Empty) +
                    (!string.IsNullOrEmpty(IndexCOLUMN) ? ",Column " + IndexCOLUMN : string.Empty) + (!string.IsNullOrEmpty(IndexTABLE) ? ",Table " + IndexTABLE : string.Empty) + (!string.IsNullOrEmpty(IndexFIGURE) ? ",Figure " + IndexFIGURE : string.Empty) +
                     (!string.IsNullOrEmpty(IndexSCHEMES) ? ",Scheme " + IndexSCHEMES : string.Empty) +
                    (!string.IsNullOrEmpty(IndexSHEET) ? ",Sheet " + IndexSHEET : string.Empty) + (!string.IsNullOrEmpty(IndexFOOTNOTE) ? ",Footnote " + IndexFOOTNOTE : string.Empty) + "," + IndexCOMMENT;

                    if (EditingComment == null)
                    {
                        if (TanComments.Where(tc => tc.Num == IndexNUM).Count() == 0)
                        {
                            AddTanComment(IndexCOMMENT, CommentType.INDEXING, IndexCOLUMN, IndexFIGURE, IndexFOOTNOTE, IndexLINE, IndexNUM, IndexPAGE, IndexPARA, IndexSCHEMES, IndexSHEET, IndexTABLE, Index);
                            ClearVM(null);
                        }
                        else
                        {
                            MessageBox.Show($"For Num {IndexNUM} comments already added. Duplicate Comments are not Allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else if (EditingComment.CommentType == CommentType.INDEXING)
                    {

                        Comments comment = (from c in TanComments.ToList()
                                            where c.Id == EditingComment.Id
                                            select c).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Comment = IndexCOMMENT;
                            comment.CommentType = CommentType.INDEXING;
                            comment.Length = Index.Length;
                            comment.Column = IndexCOLUMN;
                            comment.Figure = IndexFIGURE;
                            comment.FootNote = IndexFOOTNOTE;
                            comment.Line = IndexLINE;
                            comment.Num = IndexNUM;
                            comment.Page = IndexPAGE;
                            comment.Para = IndexPARA.ToString();
                            comment.Schemes = IndexSCHEMES;
                            comment.Sheet = IndexSHEET;
                            comment.Table = IndexTABLE;
                            comment.TotalComment = Index;
                        }
                        ClearVM(null);
                    }
                    else
                        MessageBox.Show("You Do Not Add New Comment in Edit Mode. Please Update " + EditingComment.CommentType + " First.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                    MessageBox.Show(ErrMsg, "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private bool ValidateIndexComments(out string ErrMsg)
        {
            try
            {
                bool result = true;
                ErrMsg = string.Empty;
                if (IndexNUM == 0 || string.IsNullOrEmpty(IndexPAGE) || string.IsNullOrEmpty(IndexCOMMENT))
                {
                    result = false;
                    ErrMsg = "Page,NUM,Comments are manadatory to Save TanComments";
                }
                else if (S.CommentParameters.Any(x => IndexNUM.ToString().ToLower().Contains(x)) || S.CommentParameters.Any(x => IndexPAGE.ToString().ToLower().Contains(x)) ||
                    S.CommentParameters.Any(x => IndexLINE.ToString().ToLower().Contains(x)) || S.CommentParameters.Any(x => IndexPARA.ToString().ToLower().Contains(x)) ||
                    S.CommentParameters.Any(x => IndexCOLUMN.ToString().ToLower().Contains(x)) || (!string.IsNullOrEmpty(IndexTABLE) && S.CommentParameters.Any(x => IndexTABLE.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(IndexFIGURE) && S.CommentParameters.Any(x => IndexFIGURE.ToLower().Contains(x))) || (!string.IsNullOrEmpty(IndexSCHEMES) && S.CommentParameters.Any(x => IndexSCHEMES.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(IndexSHEET) && S.CommentParameters.Any(x => IndexSHEET.ToLower().Contains(x))) || (!string.IsNullOrEmpty(IndexFOOTNOTE) && S.CommentParameters.Any(x => IndexFOOTNOTE.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(IndexCOMMENT) && S.CommentParameters.Any(x => IndexCOMMENT.ToLower().Contains(x))) || !CommentRegExp.IsMatch(IndexCOMMENT))
                {
                    result = false;
                    ErrMsg = "Comment parameters are not allowed";
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        public void AddTanComment(string Comment, CommentType CommentType, string Column, string Figure, String FootNote, string Line, int Num, string Page, string Para, string Schemes, string Sheet, string Table, string TotalComment)
        {
            TanComments.Add(new Comments
            {
                Id = Guid.NewGuid(),
                Comment = Comment,
                CommentType = CommentType,
                Length = TotalComment.Length,
                Column = Column,
                Figure = Figure,
                FootNote = FootNote,
                Line = Line,
                Num = Num,
                Page = Page,
                Para = Para,
                Schemes = Schemes,
                Sheet = Sheet,
                Table = Table,
                TotalComment = TotalComment
            });
        }

        private void AddAuthorComment(object obj)
        {
            try
            {
                string ErrMsg = string.Empty;
                if (ValidateAuthorComments(out ErrMsg))
                {
                    string Author = ("NUM " + AuthorNUM) + (IsAuthorNumCorrected ? " (corrected)" : string.Empty) + (!string.IsNullOrEmpty(AuthorPAGE) ? ",Page " + AuthorPAGE : string.Empty) + (!string.IsNullOrEmpty(AuthorLINE) ? ",Line " + AuthorLINE : string.Empty) + (!string.IsNullOrEmpty(AuthorPARA) ? ",Para " + AuthorPARA : string.Empty) +
                        (!string.IsNullOrEmpty(AuthorCOLUMN) ? ",Column " + AuthorCOLUMN : string.Empty) + (!string.IsNullOrEmpty(AuthorTABLE) ? ",Table " + AuthorTABLE : string.Empty) + (!string.IsNullOrEmpty(AuthorFIGURE) ? ",Figure " + AuthorFIGURE : string.Empty) +
                        (!string.IsNullOrEmpty(AuthorSCHEMES) ? ",Scheme " + AuthorSCHEMES : string.Empty) +
                        (!string.IsNullOrEmpty(AuthorSHEET) ? ",Sheet " + AuthorSHEET : string.Empty) + (!string.IsNullOrEmpty(AuthorFOOTNOTE) ? ",Footnote " + AuthorFOOTNOTE : string.Empty) + "," + AuthorCOMMENT;
                    if (EditingComment == null)
                    {
                        if (TanComments.Where(tc => tc.Num != 0 && tc.Num == AuthorNUM).Count() == 0)
                        {
                            AddTanComment(AuthorCOMMENT, CommentType.AUTHOR, AuthorCOLUMN, AuthorFIGURE, AuthorFOOTNOTE, AuthorLINE, AuthorNUM, AuthorPAGE, AuthorPARA, AuthorSCHEMES, AuthorSHEET, AuthorTABLE, Author);
                            ClearVM(null);
                        }
                        else
                        {
                            MessageBox.Show($"Num {AuthorNUM} comments already added. Duplicate Comments are not Allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else if (EditingComment.CommentType == CommentType.AUTHOR)
                    {
                        Comments comment = (from c in TanComments.ToList()
                                            where c.Id == EditingComment.Id
                                            select c).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Comment = AuthorCOMMENT;
                            comment.CommentType = CommentType.INDEXING;
                            comment.Length = Author.Length;
                            comment.Column = AuthorCOLUMN;
                            comment.Figure = AuthorFIGURE;
                            comment.FootNote = AuthorFOOTNOTE;
                            comment.Line = AuthorLINE;
                            comment.Num = AuthorNUM;
                            comment.Page = AuthorPAGE;
                            comment.Para = AuthorPARA.ToString();
                            comment.Schemes = AuthorSCHEMES;
                            comment.Sheet = AuthorSHEET;
                            comment.Table = AuthorTABLE;
                            comment.TotalComment = Author;
                        }
                        ClearVM(null);
                    }
                    else
                        MessageBox.Show("You Do Not Add New Comment in Edit Mode. Please Update " + EditingComment.CommentType + " First.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Page,NUM,Comments are manadatory to Save TanComments", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }


        private bool ValidateAuthorComments(out string ErrMsg)
        {

            ErrMsg = string.Empty;
            try
            {
                ;
                bool result = true;
                if (AuthorNUM == 0 || string.IsNullOrEmpty(AuthorPAGE) || string.IsNullOrEmpty(AuthorCOMMENT))
                {
                    result = false;
                    ErrMsg = "Page,NUM,Comments are manadatory to Save TanComments";
                }
                else if (S.CommentParameters.Any(x => AuthorNUM.ToString().ToLower().Contains(x)) || S.CommentParameters.Any(x => AuthorPAGE.ToString().ToLower().Contains(x)) ||
                    S.CommentParameters.Any(x => AuthorLINE.ToString().ToLower().Contains(x)) || S.CommentParameters.Any(x => AuthorPARA.ToString().ToLower().Contains(x)) ||
                    S.CommentParameters.Any(x => AuthorCOLUMN.ToString().ToLower().Contains(x)) || (!string.IsNullOrEmpty(AuthorTABLE) && S.CommentParameters.Any(x => AuthorTABLE.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(AuthorFIGURE) && S.CommentParameters.Any(x => AuthorFIGURE.ToLower().Contains(x))) || (!string.IsNullOrEmpty(AuthorSCHEMES) && S.CommentParameters.Any(x => AuthorSCHEMES.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(AuthorSHEET) && S.CommentParameters.Any(x => AuthorSHEET.ToLower().Contains(x))) || (!string.IsNullOrEmpty(AuthorFOOTNOTE) && S.CommentParameters.Any(x => AuthorFOOTNOTE.ToLower().Contains(x))) ||
                    (!string.IsNullOrEmpty(AuthorCOMMENT) && S.CommentParameters.Any(x => AuthorCOMMENT.ToLower().Contains(x))) || !CommentRegExp.IsMatch(AuthorCOMMENT))
                {
                    result = false;
                    ErrMsg = "Comment parameters are not allowed";
                }
                ;
                return result;
            }
            catch (Exception ex)
            {
                Log.This(ex);

                return false;
            }
        }

        private void AddOtherComment(object obj)
        {

            try
            {
                if (!string.IsNullOrEmpty(OtherCOMMENT))
                {
                    string Other = OtherNUM != 0 ? $"NUM {OtherNUM} {OtherCOMMENT}" : OtherCOMMENT;
                    if (EditingComment == null)
                    {
                        if (TanComments.Where(tc => tc.Num != 0 && tc.Num == OtherNUM).Count() == 0)
                        {
                            TanComments.Add(new Comments
                            {
                                Id = Guid.NewGuid(),
                                Num = OtherNUM,
                                Comment = OtherCOMMENT,
                                CommentType = CommentType.OTHER,
                                Length = Other.Length,
                                TotalComment = Other
                            });
                            ClearVM(null);
                        }
                        else
                        {
                            MessageBox.Show($"For Num {OtherNUM} comments already added. Duplicate Comments are not Allowed", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (EditingComment.CommentType == CommentType.OTHER)
                    {
                        Comments comment = (from c in TanComments.ToList()
                                            where c.Id == EditingComment.Id
                                            select c).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Comment = OtherCOMMENT;
                            comment.CommentType = CommentType.OTHER;
                            comment.TotalComment = Other;
                            comment.Length = Other.Length;
                            comment.Num = OtherNUM;
                            ClearVM(null);
                        }
                    }
                    else
                        MessageBox.Show("You Do Not Add New Comment in Edit Mode. Please Update " + EditingComment.CommentType + " First.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Comments are manadatory to Save TanComments", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void AddCASComment(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(CasConsultedCOMMENT))
                {
                    if (EditingComment == null)
                    {
                        TanComments.Add(new Comments { Id = Guid.NewGuid(), Comment = CasConsultedCOMMENT, CommentType = CommentType.CAS, Length = CasConsultedCOMMENT.Length, TotalComment = CasConsultedCOMMENT });
                        ClearVM(null);
                    }
                    else if (EditingComment.CommentType == CommentType.CAS)
                    {
                        Comments comment = (from c in TanComments.ToList()
                                            where c.Id == EditingComment.Id
                                            select c).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Comment = CasConsultedCOMMENT;
                            comment.Length = CasConsultedCOMMENT.Length;
                            comment.TotalComment = CasConsultedCOMMENT;
                            comment.CommentType = CommentType.CAS;
                            ClearVM(null);
                        }
                    }
                    else
                        MessageBox.Show("You Do Not Add New Comment in Edit Mode. Please Update " + EditingComment.CommentType + " First.", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Comments are manadatory to Save TanComments", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void EditComment(object obj)
        {

            try
            {
                ;
                if (SelectedComment != null)
                {

                    EditingComment = SelectedComment;
                    SelectedTab = Array.IndexOf(Enum.GetValues(SelectedComment.CommentType.GetType()), SelectedComment.CommentType);
                    if (SelectedComment.CommentType == CommentType.INDEXING)
                    {
                        SelectedTab = 0;
                        // List<String> list = SelectedComment.Comment.Split(',').ToList();
                        IndexNUM = SelectedComment.Num;
                        IndexCOLUMN = SelectedComment.Column;
                        IndexFIGURE = SelectedComment.Figure;
                        IndexFOOTNOTE = SelectedComment.FootNote;
                        IndexLINE = SelectedComment.Line;
                        IndexPAGE = SelectedComment.Page;
                        IndexPARA = SelectedComment.Para;
                        IndexSCHEMES = SelectedComment.Schemes;
                        IndexSHEET = SelectedComment.Sheet;
                        IndexTABLE = SelectedComment.Table;
                        IndexCOMMENT = SelectedComment.Comment;
                        #region Commented
                        //foreach (var item in list)
                        //{
                        //    if (item.Contains("NUM"))
                        //    {
                        //        IndexNUM = Convert.ToInt16(item.Replace("NUM ", ""));
                        //    }
                        //    else if (item.Contains("Page"))
                        //    {
                        //        IndexPAGE = Convert.ToInt16(item.Replace("Page ", ""));
                        //    }
                        //    else if (item.Contains("Line"))
                        //    {
                        //        IndexLINE = Convert.ToInt16(item.Replace("Line ", ""));
                        //    }
                        //    else if (item.Contains("Para"))
                        //    {
                        //        IndexPARA = Convert.ToInt16(item.Replace("Para ", ""));
                        //    }
                        //    else if (item.Contains("Column"))
                        //    {
                        //        IndexCOLUMN = Convert.ToInt16(item.Replace("Column ", ""));
                        //    }
                        //    else if (item.Contains("Table"))
                        //    {
                        //        IndexTABLE = item.Replace("Table ", "");
                        //    }
                        //    else if (item.Contains("Figure"))
                        //    {
                        //        IndexFIGURE = item.Replace("Figure ", "");
                        //    }
                        //    else if (item.Contains("Scheme"))
                        //    {
                        //        IndexSCHEMES = item.Replace("Scheme ", "");
                        //    }
                        //    else if (item.Contains("Sheet"))
                        //    {
                        //        IndexSHEET = item.Replace("Sheet ", "");
                        //    }
                        //    else if (item.Contains("Footnote"))
                        //    {
                        //        IndexFOOTNOTE = item.Replace("Footnote ", "");
                        //    }
                        //    else
                        //    {
                        //        IndexCOMMENT = item;
                        //    }

                        //} 
                        #endregion
                    }

                    else if (SelectedComment.CommentType == CommentType.AUTHOR)
                    {
                        #region Commented
                        //List<String> list = SelectedComment.Comment.Split(',').ToList();
                        //foreach (var item in list)
                        //{
                        //    if (item.Contains("NUM"))
                        //    {
                        //        IsAuthorNumCorrected = item.Contains("(corrected)") ? true : false;
                        //        AuthorNUM = Convert.ToInt16(item.Replace("NUM ", "").Replace("(corrected)", ""));
                        //    }
                        //    else if (item.Contains("Page"))
                        //    {
                        //        AuthorPAGE = Convert.ToInt16(item.Replace("Page ", ""));
                        //    }
                        //    else if (item.Contains("Line"))
                        //    {
                        //        AuthorLINE = Convert.ToInt16(item.Replace("Line ", ""));
                        //    }
                        //    else if (item.Contains("Para"))
                        //    {
                        //        AuthorPARA = Convert.ToInt16(item.Replace("Para ", ""));
                        //    }
                        //    else if (item.Contains("Column"))
                        //    {
                        //        AuthorCOLUMN = Convert.ToInt16(item.Replace("Column ", ""));
                        //    }
                        //    else if (item.Contains("Table"))
                        //    {
                        //        AuthorTABLE = item.Replace("Table ", "");
                        //    }
                        //    else if (item.Contains("Figure"))
                        //    {
                        //        AuthorFIGURE = item.Replace("Figure ", "");
                        //    }
                        //    else if (item.Contains("Scheme"))
                        //    {
                        //        AuthorSCHEMES = item.Replace("Scheme ", "");
                        //    }
                        //    else if (item.Contains("Sheet"))
                        //    {
                        //        AuthorSHEET = item.Replace("Sheet ", "");
                        //    }
                        //    else if (item.Contains("Footnote"))
                        //    {
                        //        AuthorFOOTNOTE = item.Replace("Footnote ", "");
                        //    }
                        //    else
                        //    {
                        //        AuthorCOMMENT = item;
                        //    }
                        //    SelectedTab = 1;
                        //} 
                        #endregion
                        AuthorNUM = SelectedComment.Num;
                        AuthorCOLUMN = SelectedComment.Column;
                        AuthorFIGURE = SelectedComment.Figure;
                        AuthorFOOTNOTE = SelectedComment.FootNote;
                        AuthorLINE = SelectedComment.Line;
                        AuthorPAGE = SelectedComment.Page;
                        AuthorPARA = SelectedComment.Para;
                        AuthorSCHEMES = SelectedComment.Schemes;
                        AuthorSHEET = SelectedComment.Sheet;
                        AuthorTABLE = SelectedComment.Table;
                        AuthorCOMMENT = SelectedComment.Comment;
                    }

                    else if (SelectedComment.CommentType == CommentType.OTHER)
                    {
                        OtherNUM = SelectedComment.Num;
                        OtherCOMMENT = SelectedComment.Comment;
                        SelectedTab = 2;
                    }
                    else if (SelectedComment.CommentType == CommentType.DEFAULT)
                    {
                        DefaultCOMMENT = SelectedComment.Comment;
                        SelectedTab = 3;
                    }
                    else if (SelectedComment.CommentType == CommentType.TEMPERATURE)
                    {
                        TemperatureCOMMENT = SelectedComment.Comment;
                        SelectedTab = 3;
                    }
                    else
                    {
                        CasConsultedCOMMENT = SelectedComment.Comment;
                        SelectedTab = 4;
                    }
                }
                else
                    MessageBox.Show("Please Select Comment To Edit", "Reactions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearVM(object obj)
        {
            try
            {
                CommentLegth = (from c in TanComments where c.CommentType != CommentType.DEFAULT select c.Length).ToList().Sum();
                AuthorNUM = 0;
                AuthorPAGE = string.Empty;
                AuthorLINE = string.Empty;
                AuthorPARA = string.Empty;
                AuthorCOLUMN = string.Empty;
                AuthorTABLE = string.Empty;
                AuthorFIGURE = string.Empty;
                AuthorSCHEMES = string.Empty;
                AuthorSHEET = string.Empty;
                AuthorFOOTNOTE = string.Empty;
                AuthorCOMMENT = string.Empty;
                IsAuthorNumCorrected = false;

                IndexNUM = 0;
                IndexPAGE = string.Empty;
                IndexLINE = string.Empty;
                IndexPARA = string.Empty;
                IndexCOLUMN = string.Empty;
                IndexTABLE = string.Empty;
                IndexFIGURE = string.Empty;
                IndexSCHEMES = string.Empty;
                IndexSHEET = string.Empty;
                IndexFOOTNOTE = string.Empty;
                IndexCOMMENT = string.Empty;
                OtherNUM = 0;
                OtherCOMMENT = string.Empty;
                CasConsultedCOMMENT = string.Empty;
                TemperatureCOMMENT = string.Empty;
                DefaultCOMMENT = string.Empty;

                EditingComment = null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }

    public class Comments : OrderableVM
    {
        private Guid id;
        private string comment;
        private CommentType commentType;
        private int length;

        private int num;
        private string page;
        private string para;
        private string line;
        private string column;
        private string table;
        private string figure;
        private string schemes;
        private string sheet;
        private string footNote;
        private string totalComment;
        private int displayOrder;

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public string Comment
        {
            get { return comment; }
            set
            {
                SetProperty(ref comment, value);
            }
        }
        public CommentType CommentType { get { return commentType; } set { SetProperty(ref commentType, value); } }
        public int Length { get { return length; } set { SetProperty(ref length, value); } }

        public string Page { get { return page; } set { SetProperty(ref page, value); } }
        public int Num { get { return num; } set { SetProperty(ref num, value); } }
        public string Para { get { return para; } set { SetProperty(ref para, value); } }
        public string Line { get { return line; } set { SetProperty(ref line, value); } }
        public string Column { get { return column; } set { SetProperty(ref column, value); } }
        public string Table { get { return table; } set { SetProperty(ref table, value); } }
        public string Figure { get { return figure; } set { SetProperty(ref figure, value); } }
        public string Schemes { get { return schemes; } set { SetProperty(ref schemes, value); } }
        public string Sheet { get { return sheet; } set { SetProperty(ref sheet, value); } }
        public string FootNote { get { return footNote; } set { SetProperty(ref footNote, value); } }
        public string TotalComment
        {
            get { return totalComment; }
            set
            {
                SetProperty(ref totalComment, value);
                Length = value.Length;
            }
        }
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }

    }
}
