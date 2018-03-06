using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    public enum ErrorCodes
    {
        UNKNOWN_ERROR = 0,

        #region Tool Opening
        CVT_DICT_NOT_FOUND = 1,
        CVT_DICT_CANT_LOADED = 2,
        FREE_TEXT_DICT_NOT_FOUND = 3,
        FREE_TEXT_DICT_CANT_LOADED = 4,
        USER_ROLES_CANT_LOADED = 5,
        CANT_CONNECT_TO_SERVER = 6,
        USER_PASSWORD_WRONG = 7,
        S8500_IMAGES_NOT_FOUND = 8,
        S9000_IMAGES_NOT_FOUND = 9,
        #endregion

        #region Users & Tasks
        NO_TASKS_FOUND = 101,
        ERROR_WHILE_LOADING_TASK_SHEET = 102,
        CANT_GET_TAN_DETAILS = 103,
        CANT_FETCH_USER_INFO = 104,
        ERROR_WHILE_LOADING_TAN = 105,
        NUM_IMAGES_NOT_FOUND = 106,
        #endregion

        #region Pdf related
        PDF_NOT_FOUND = 201,
        PDF_CANT_BE_OPEN = 202,
        PDF_CANT_BE_SAVED = 203,
        FILE_IS_LOCKED = 204,
        #endregion

        #region Participants Related
        CANT_SHOW_CHEMICALS_DIALOG = 301,
        CANT_ADD_PARTICIPANT = 302,
        CANT_GENERATE_8500_NUM = 303,
        CANT_GENERATE_8000_NUM = 304,
        CANT_GET_IMAGE_FROM_MOLE = 305,
        CANT_OPEN_CHEMICAL_SOFTWARE = 306,
        CANT_DELETE_CHEMICAL = 307,
        CANT_DELETE_PARTICIPANT = 308,
        CANT_EDIT_8500_NUM = 309,
        CANT_EDIT_8000_NUM = 310,
        CANT_SEARCH_IN_BROWSER = 311,
        #endregion

        #region RSN related
        CANT_LOAD_RSN_DIALOG = 501,
        CANT_ADD_RSN_IN_DIALOG = 502,
        CANT_EDIT_RSN = 503,
        CANT_SUBMIT_RSN_TO_WINDOW = 504
        #endregion
    }
}
