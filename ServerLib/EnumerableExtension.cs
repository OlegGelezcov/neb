// EnumerableExtension.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Tuesday, November 4, 2014 5:34:56 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class EnumerableExtension
    {
        public static IEnumerable<T> AsLocked<T>(this IEnumerable<T> ie, object @lock )
        {
            return new SafeEnumerable<T>(ie, @lock);
        }
    }
}

