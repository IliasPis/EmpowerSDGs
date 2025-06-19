using System.Collections;
using UnityEngine;

public class BlockComplete : MonoBehaviour
{
    public bool isVisited = false; // Χρησιμοποιείται για να ελέγχεται αν το block έχει ήδη επισκεφτεί

    // Αυτό το GameObject θα αναφέρεται στο pawn που αλληλεπιδρά με το block (αν χρειάζεται για άλλες λειτουργίες)
    public GameObject pawn;

    // Μέθοδος για να ενημερώνουμε ότι το block έχει επισκεφτεί
    public void MarkAsVisited()
    {
        isVisited = true;
    }
}
