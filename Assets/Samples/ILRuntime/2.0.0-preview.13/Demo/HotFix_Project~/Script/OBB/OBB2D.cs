using UnityEngine;

namespace RedRedJiang.Unity
{
    /// <summary>
    /// 自制 OBB 包围盒
    /// </summary>
    public class OBB2D
    {

        /** Corners of the box, where 0 is the lower left. */
        private Vector2[] corner = new Vector2[4];
        public Vector2[] Corner { get { return corner; } }

        /** Two edges of the box extended away from corner[0]. */
        private Vector2[] axis = new Vector2[2];
        public Vector2[] Axis { get { return axis; } }

        /** origin[a] = corner[0].dot(axis[a]); */
        private float[] origin = new float[2];
        public float[] Origin { get { return origin; } }

        /** Returns true if other overlaps one dimension of this. */
        private bool overlaps1Way(OBB2D other)
        {
            for (int a = 0; a < 2; ++a)
            {

                float t = Vector2.Dot(other.corner[0], axis[a]);

                // Find the extent of box 2 on axis a
                float tMin = t;
                float tMax = t;

                for (int c = 1; c < 4; ++c)
                {
                    t = Vector2.Dot(other.corner[c], axis[a]);

                    if (t < tMin)
                    {
                        tMin = t;
                    }
                    else if (t > tMax)
                    {
                        tMax = t;
                    }
                }

                // We have to subtract off the origin

                // See if [tMin, tMax] intersects [0, 1]
                if ((tMin > 1 + origin[a]) || (tMax < origin[a]))
                {
                    // There was no intersection along this dimension;
                    // the boxes cannot possibly overlap.
                    return false;
                }
            }

            // There was no dimension along which there is no intersection.
            // Therefore the boxes overlap.
            return true;
        }


        /** Updates the axes after the corners move.  Assumes the
            corners actually form a rectangle. */
        private void computeAxes()
        {
            axis[0] = corner[1] - corner[0];
            axis[1] = corner[3] - corner[0];

            // Make the length of each axis 1/edge length so we know any
            // dot product must be less than 1 to fall within the edge.

            for (int a = 0; a < 2; ++a)
            {
                axis[a] /= Vector2.SqrMagnitude(axis[a]);
                origin[a] = Vector2.Dot(corner[0], axis[a]);
            }
        }



        public OBB2D(Vector2 center, float w, float h, float angle, Vector2 offect)
        {
            Vector2 X = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 Y = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

            X *= w / 2;
            Y *= h / 2;

            center += offect;

            corner[0] = center - X - Y;
            corner[1] = center + X - Y;
            corner[2] = center + X + Y;
            corner[3] = center - X + Y;

            //corner[0] = center - Y;
            //corner[1] = center +2* X -  Y;
            //corner[2] = center + 2* X +  Y;
            //corner[3] = center + Y;

            computeAxes();
        }


        /** For testing purposes. */
        //public void moveTo(Vector2 center)
        //{
        //    Vector2 centroid = (corner[0] + corner[1] + corner[2] + corner[3]) / 4;

        //    Vector2 translation = center - centroid;

        //    for (int c = 0; c < 4; ++c)
        //    {
        //        corner[c] += translation;
        //    }

        //    computeAxes();
        //}

        /** Returns true if the intersection of the boxes is non-empty. */
        public bool overlaps(OBB2D other)
        {
            return overlaps1Way(other) && other.overlaps1Way(this);
        }

        //绘画功能

        //     public void render() {
        //         glBegin(GL_LINES);
        //             for (int c = 0; c< 5; ++c) {
        //               glVertex2fv(corner[c & 3]);
        //             }
        //         glEnd();
        //     }
    }
}

