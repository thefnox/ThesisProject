﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    static class Constants
    {
        public const byte INPUT_BUFFER_SIZE = 6;
        public const short COUNTER_HIT_BONUS = 125;
        public const short GAME_BUFFER_SIZE = 9001;
        public const short GAME_TIME = 99 * 60;
        public const int START_POS_X = 400;
        public const int START_POS_Y = 0;
        public const int BOUNDARY_X = 700;
        public const int BOUNDARY_Y = 500;
        public const int GRAVITY = 1000;
        // As we're using integers only, deacceleration by gravity needs to be precalculated
        public static int[] GRAVITY_VALUES = new int[]
        {
            0,
            1,
            2,
            5,
            9,
            14,
            20,
            27,
            36,
            45,
            56,
            67,
            80,
            94,
            109,
            125,
            142,
            161,
            180,
            201,
            222,
            245,
            269,
            294,
            320,
            347,
            376,
            405,
            436,
            467,
            500,
            534,
            569,
            605,
            642,
            681,
            720,
            761,
            802,
            845,
            889,
            934,
            980,
            1027,
            1076,
            1125,
            1176,
            1227,
            1280,
            1334,
            1389,
            1445,
            1502,
            1561,
            1620,
            1681,
            1742,
            1805,
            1869,
            1934,
            2000,
            2067,
            2136,
            2205,
            2276,
            2347,
            2420,
            2494,
            2569,
            2645,
            2722,
            2801,
            2880,
            2961,
            3042,
            3125,
            3209,
            3294,
            3380,
            3467,
            3556,
            3645,
            3736,
            3827,
            3920,
            4014,
            4109,
            4205,
            4302,
            4401,
            4500,
            4601,
            4702,
            4805,
            4909,
            5014,
            5120,
            5227,
            5336,
            5445,
            5556,
            5667,
            5780,
            5894,
            6009,
            6125,
            6242,
            6361,
            6480,
            6601,
            6722,
            6845,
            6969,
            7094,
            7220,
            7347,
            7476,
            7605,
            7736,
            7867,
            8000
        };
    }
}
