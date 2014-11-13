using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoggleChallenge.Models;

namespace BoggleChallenge.Controllers
{
    public class ChallengeController : Controller
    {
        [Route("Challenges/LinkedList")]
        public ActionResult LinkedListChallenge(int? linkedListSize)
        {
            if (linkedListSize.HasValue)
            {
                Node listHead = Node.MakeRandomLinkedList(linkedListSize.Value);
                ViewBag.OriginalString = listHead.ToString();
                Node copyHead = Node.DuplicateList(listHead);
                ViewBag.CopyString = copyHead.ToString();
                ViewBag.HasLL = true;
            }
            else
            {
                ViewBag.HasLL = false;
            }

            return View();
        }

        [Route("Challenges/BoggleBoard")]
        public ActionResult BoggleBoardChallenge(string board, int? rows, int? cols)
        {
            // If all fields are filled in
            if (!String.IsNullOrEmpty(board) && rows.HasValue && cols.HasValue)
            {
                char[,] actualBoard;
                if (VerifyInput(board, rows.Value, cols.Value, out actualBoard))
                {
                    var wordsInBoard = BoggleBoard.SolveBoggleBoard(actualBoard, rows.Value, cols.Value);

                    ViewBag.HasBoggle = true;
                    ViewBag.WordList = wordsInBoard;
                    ViewBag.Board = board;
                    ViewBag.Error = false;
                }
                else
                {
                    ViewBag.HasBoggle = false;
                    ViewBag.Error = true;
                }

            }
            // if all fields are empty
            else if (String.IsNullOrEmpty(board) && !rows.HasValue && !cols.HasValue)
            {
                ViewBag.HasBoggle = false;
                ViewBag.Error = false;
            }
            // If one field (but not all) is empty
            else if ((String.IsNullOrEmpty(board) || !rows.HasValue || !cols.HasValue) && !(!String.IsNullOrEmpty(board) && rows.HasValue && cols.HasValue))
            {
                ViewBag.HasBoggle = false;
                ViewBag.Error = true;
            }

            return View();
        }

        protected bool VerifyInput(string board, int m, int n, out char[,] actualBoard)
        {
            board = board.ToLower();
            board = board.Replace("\r", "");
            string[] rows = board.Split('\n');
            actualBoard = new char[m, n];
            // First check - correct number of rows
            if (rows.Length != m) return false;

            int rowNum = 0;
            foreach (string row in rows)
            {
                int colNum = 0;
                string[] squares = row.Split(' ');
                // Second check - correct number of columns
                if (squares.Length != n) return false;
                foreach (string letter in squares)
                {
                    if (String.IsNullOrEmpty(letter)) return false; // Check for extra whitespace
                    // Could check for > 1 character per square, but let's ignore it instead. It's an easy typo to make.
                    actualBoard[rowNum, colNum] = letter[0];
                    colNum++;
                }
                rowNum++;
            }

            return true;
        }
    }
}