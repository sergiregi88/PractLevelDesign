using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class PathDefinition : MonoBehaviour {

    // Array Points Of Path
    public Transform[] Points;
    //Function returns IEnumerator to go over the path and return index of path every time function is called
    public IEnumerator<Transform> GetPathsEnumerator()
    {
        // if path is null or Path length is minor than 1 then terminate sequence 
        // because if we don't have points of lenth of points is minus than 1 no have sequence
        if (Points == null || Points.Length < 1)
            yield break;

        var direction = 1;
        var index = 0;
        //loop infinite
        while (true)
        {
            //return current position 
            yield return Points[index];
            // if Points length quals 1 
            // because if Points.Length == 1 and index=0 enter in if beacuse index=0 and enter in else if because Points.Length-1 is 0 
            // direction become -1 and index be 0=0-1 = -1 and the yield Return Points[-1] that position not exist 
            if (Points.Length == 1)
                continue;
            //if index equals minor 0 or direction positive
            if (index <= 0)
                direction = 1;
            // if index equals grader Points lenght -1 or index is at the end in Point Array direction negative
            else if (index >= Points.Length - 1)
                direction = -1;
            // add or substatract direction from index
            index = index + direction;
        }

        // amb varis tutorials esta fet 
        // un x fisiques raycast 
        // aixo q fem a mates 
        // les matg son  un pakage de jocs q hi ha x internet x fer proves 
        // tambe faibg q si per exemple el jugador passa un punt hi mort no haigi de tornar al prinipi començi desde el punt 
    }
    // arriben a 50km/h sobre aigua 
    // draw line of path in scene 
    public void OnDrawGizmos()
    {

        // if Points is null or Points Length is min 2 return
        if (Points == null || Points.Length < 2)
            return;
        // get Points not null in a list
        var points = Points.Where(t => t != null).ToList();
        // if points Count is < 2 then return
        if (points.Count < 2)
            return;
        // we start index 1 loop at 2on position of Points because we draw line from previous point to the current point
        // loop for draw lines in scene
        for (var i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }
    
}
