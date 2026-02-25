using System;
using System.Collections.Generic;
using UnityEngine;

namespace TriInkTrack.Drawing
{
    public class InkLinePool : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InkLine inkLinePrefab;
        [SerializeField] private Transform lineRoot;

        [Header("Pool")]
        [SerializeField] private int initialPoolSize = 10;

        private readonly Queue<InkLine> availableLines = new Queue<InkLine>(32);
        private readonly HashSet<InkLine> pooledLines = new HashSet<InkLine>();
        private bool prewarmed;

        public event Action<InkLine> OnLineReturned;

        private void Awake()
        {
            if (lineRoot == null)
            {
                lineRoot = transform;
            }
        }

        private void Start()
        {
            PrewarmIfNeeded();
        }

        public void Configure(InkLine prefab, Transform root)
        {
            if (prefab != null)
            {
                inkLinePrefab = prefab;
            }

            if (root != null)
            {
                lineRoot = root;
            }
        }

        public InkLine Get()
        {
            PrewarmIfNeeded();

            while (availableLines.Count > 0)
            {
                InkLine line = availableLines.Dequeue();
                if (line == null)
                {
                    continue;
                }

                pooledLines.Remove(line);
                PrepareLineForUse(line);
                return line;
            }

            InkLine created = CreateLineInstance();
            PrepareLineForUse(created);
            return created;
        }

        public void Return(InkLine line)
        {
            ReturnInternal(line, true);
        }

        private void PrewarmIfNeeded()
        {
            if (prewarmed || initialPoolSize <= 0)
            {
                prewarmed = true;
                return;
            }

            prewarmed = true;
            int count = Mathf.Max(0, initialPoolSize);
            for (int i = 0; i < count; i++)
            {
                InkLine line = CreateLineInstance();
                ReturnInternal(line, false);
            }
        }

        private void PrepareLineForUse(InkLine line)
        {
            if (line == null)
            {
                return;
            }

            if (lineRoot != null)
            {
                line.transform.SetParent(lineRoot, false);
            }

            line.gameObject.SetActive(true);
            line.transform.localPosition = Vector3.zero;
            line.transform.localRotation = Quaternion.identity;
            line.transform.localScale = Vector3.one;
            line.ResetLine();

            InkLineLifetime lifetime = EnsureLifetime(line);
            lifetime.PrepareForReuse();
        }

        private void ReturnInternal(InkLine line, bool notify)
        {
            if (line == null || pooledLines.Contains(line))
            {
                return;
            }

            InkLineLifetime lifetime = EnsureLifetime(line);
            lifetime.StopLifetime();

            line.ResetLine();
            if (lineRoot != null)
            {
                line.transform.SetParent(lineRoot, false);
            }

            line.gameObject.SetActive(false);
            availableLines.Enqueue(line);
            pooledLines.Add(line);

            if (notify)
            {
                OnLineReturned?.Invoke(line);
            }
        }

        private InkLine CreateLineInstance()
        {
            Transform parent = lineRoot != null ? lineRoot : transform;

            InkLine line;
            if (inkLinePrefab != null)
            {
                line = Instantiate(inkLinePrefab, parent);
            }
            else
            {
                GameObject runtimeLine = new GameObject("InkLine_Runtime");
                runtimeLine.transform.SetParent(parent, false);
                runtimeLine.AddComponent<LineRenderer>();
                runtimeLine.AddComponent<EdgeCollider2D>();
                line = runtimeLine.AddComponent<InkLine>();
            }

            EnsureLifetime(line);
            return line;
        }

        private static InkLineLifetime EnsureLifetime(InkLine line)
        {
            InkLineLifetime lifetime = line.GetComponent<InkLineLifetime>();
            if (lifetime == null)
            {
                lifetime = line.gameObject.AddComponent<InkLineLifetime>();
            }

            return lifetime;
        }
    }
}
