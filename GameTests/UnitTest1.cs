using GameServer.Physics;
using GameServer.MapObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using GameServer.Models;

namespace GameTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CubeSphereIntersection1()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, 2);
            MapBox o2 = new MapBox(0, 0, 20, 1, 1, 1);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsFalse(intersects, "Intersection error sphere and box");
        }

        [TestMethod]
        public void CubeSphereIntersection2()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, 2);
            MapBox o2 = new MapBox(0, 0, 1, 1, 1, 1);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsTrue(intersects, "Intersection error sphere and box");
        }

        [TestMethod]
        public void CubeSphereIntersection3()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, 2);
            MapBox o2 = new MapBox(0, 2, 2, 1, 1, 1);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsFalse(intersects, "Intersection error sphere and box");
        }


        [TestMethod]
        public void SphereSphereIntersection1()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, 2);
            MapSphere o2 = new MapSphere(0, 0, 1, 0.1f);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsTrue(intersects, "Intersection error sphere and box");
        }


        [TestMethod]
        public void SphereSphereIntersection2()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, 2);
            MapSphere o2 = new MapSphere(0, 0, 2, 2);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsFalse(intersects, "Intersection error sphere and box");
        }


        [TestMethod]
        public void SphereSphereIntersection3()
        {
            // arrange  
            MapSphere o1 = new MapSphere(0, 0, 0, (float)Math.Sqrt(2) * 2f - 0.01f);
            MapSphere o2 = new MapSphere(2, 0, 2, (float)Math.Sqrt(2) * 2f);

            // act  
            bool intersects = Intersection.CheckIntersection(o2, o1);

            // assert  
            Assert.IsFalse(intersects, "Intersection error sphere and box");
        }


        [TestMethod]
        public void PointBoxIntersection1()
        {
            // arrange  
            MapBox box = new MapBox(0, 0, 0, 2, 2, 2);
            Vector3 pos = new Vector3(1, 1, 1);

            // act  
            bool intersects = Intersection.CheckIntersection(box, pos);

            // assert  
            Assert.IsTrue(intersects, "Intersection error position and box");
        }

        [TestMethod]
        public void PointBoxIntersection2()
        {
            // arrange  
            MapBox box = new MapBox(0, 0, 0, 2, 2, 2);
            Vector3 pos = new Vector3(2, 0, 0);

            // act  
            bool intersects = Intersection.CheckIntersection(box, pos);

            // assert  
            Assert.IsFalse(intersects, "Intersection error position and box");
        }

        [TestMethod]
        public void BoxRayIntersection1()
        {
            // arrange  
            MapBox box = new MapBox(10, 0, 0, 1, 1, 1);
            Ray ray = new Ray(0, 0, 0, 1, 0, 0);

            // act  
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void BoxRayIntersection2()
        {
            // arrange  
            MapBox box = new MapBox(0, 0, 1, 2, 2, 2);
            Ray ray = new Ray(0, 0, -1, 0, 0, 1);

            // act  
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void BoxRayIntersection3()
        {
            // arrange  
            MapBox box = new MapBox(0, 3, 1, 2, 2, 2);
            Ray ray = new Ray(0, 0, -1, 0, 0, 1);

            // act 
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsFalse(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void BoxRayIntersection4()
        {
            // arrange  
            MapBox box = new MapBox(2, 0, 2, 2, 2, 2);
            Ray ray = new Ray(-2, 0.5f, 1, 2, 0, 1);

            // act 
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void BoxRayIntersection5()
        {
            // arrange  
            MapBox box = new MapBox(2, 0, 2, 2, 2, 2);
            Ray ray = new Ray(5, 0.75f, 3, -2, -0.3f, -2);
            // act 
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void BoxRayIntersection6()
        {
            // arrange  
            MapBox box = new MapBox(2, 0, 2, 2, 2, 2);
            Ray ray = new Ray(5, 0.3f, 3, -2f, 0, -2.1f);
            // act 
            bool intersects = Intersection.CheckIntersection(box, ray, out Vector3 point);

            // assert  
            Assert.IsFalse(intersects, "Intersection error ray and box");
        }

        [TestMethod]
        public void TriangleRayIntersection1()
        {
            // arrange  
            Vector3 T0 = new Vector3(-2.0f, -2.0f, 3.0f);
            Vector3 T1 = new Vector3(2.0f, -2.0f, 3.0f);
            Vector3 T2 = new Vector3(-2.0f, 6.0f, 3.0f);

            MapTriangle triangle = new MapTriangle(T0, T1, T2);

            Ray ray = new Ray(0, 0, -1.0f, 0, 0, 1.0f);

            // act 
            bool intersects = Intersection.CheckIntersection(triangle, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and triangle");
        }

        [TestMethod]
        public void TriangleRayIntersection2()
        {
            // arrange  
            Vector3 T0 = new Vector3(-2.0f, -2.0f, 3.0f);
            Vector3 T1 = new Vector3(2.0f, -2.0f, 3.0f);
            Vector3 T2 = new Vector3(-2.0f, 6.0f, 3.0f);

            MapTriangle triangle = new MapTriangle(T0, T1, T2);

            Ray ray = new Ray(2, 0, -1.0f, 0, 0, 1.0f);

            // act 
            bool intersects = Intersection.CheckIntersection(triangle, ray, out Vector3 point);

            // assert  
            Assert.IsFalse(intersects, "Intersection error ray and triangle");
        }

        [TestMethod]
        public void QuadRayIntersection1()
        {
            // arrange  
            Vector3 Q0 = new Vector3(1, -2, 4);
            Vector3 Q1 = new Vector3(2, -2, 3);
            Vector3 Q2 = new Vector3(2, 2, 3);
            Vector3 Q3 = new Vector3(1, 2, 4);

            MapQuad quad = new MapQuad(Q0, Q1, Q2, Q3);

            Ray ray = new Ray(-2, 0, 0, 1, 0, 1);

            // act 
            bool intersects = Intersection.CheckIntersection(quad, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and quad");
        }
        [TestMethod]
        public void QuadRayIntersection2()
        {
            // arrange  
            Vector3 Q0 = new Vector3(1, -2, 4);
            Vector3 Q1 = new Vector3(2, -2, 3);
            Vector3 Q2 = new Vector3(2, 2, 3);
            Vector3 Q3 = new Vector3(1, 2, 4);

            MapQuad quad = new MapQuad(Q0, Q1, Q2, Q3);

            Ray ray = new Ray(3, 0, 0, -1, 0, 2);

            // act 
            bool intersects = Intersection.CheckIntersection(quad, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and quad");
        }
        [TestMethod]
        public void QuadRayIntersection3()
        {
            // arrange  
            Vector3 Q0 = new Vector3(1, -2, 4);
            Vector3 Q1 = new Vector3(2, -2, 3);
            Vector3 Q2 = new Vector3(2, 2, 3);
            Vector3 Q3 = new Vector3(1, 2, 4);

            MapQuad quad = new MapQuad(Q0, Q3, Q2, Q1);

            Ray ray = new Ray(-2, 0, 0, 1, 0, 1);

            // act 
            bool intersects = Intersection.CheckIntersection(quad, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and quad");
        }

        [TestMethod]
        public void QuadRayIntersection4()
        {
            // arrange  
            Vector3 Q0 = new Vector3(-2, -2, 2);
            Vector3 Q1 = new Vector3(-2, 2, 2);
            Vector3 Q2 = new Vector3(2, 2, 2);
            Vector3 Q3 = new Vector3(2, -2, 2);

            MapQuad quad = new MapQuad(Q0, Q1, Q2, Q3);

            Ray ray = new Ray(2, 0, 0, 0, 0, 1);

            // act 
            bool intersects = Intersection.CheckIntersection(quad, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and quad");
        }

        [TestMethod]
        public void QuadRayIntersection5()
        {
            // arrange  
            Vector3 Q0 = new Vector3(-3, -2, 3);
            Vector3 Q1 = new Vector3(-3, 2, 3);
            Vector3 Q2 = new Vector3(3, 2, -3);
            Vector3 Q3 = new Vector3(3, -2, -3);

            MapQuad quad = new MapQuad(Q0, Q1, Q2, Q3);

            Ray ray = new Ray(3, 0, 3, -1, -0.1f, -3);

            // act 
            bool intersects = Intersection.CheckIntersection(quad, ray, out Vector3 point);

            // assert  
            Assert.IsTrue(intersects, "Intersection error ray and quad");
        }
    }
}
