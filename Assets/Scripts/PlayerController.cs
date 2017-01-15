﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Character character;
	Iso iso;

	void Awake() {
		if (character == null)
			character = GameObject.FindWithTag("Player").GetComponent<Character>();
		SetCharacter(character);
	}

	void Start () {
	}

	void SetCharacter (Character character) {
		this.character = character;
		iso = character.GetComponent<Iso>();
	}

	void Update () {
		Vector3 targetTile;
		if (Usable.hot != null) {
			targetTile = Iso.MapToIso(Usable.hot.transform.position);
		} else {
			targetTile = Iso.MouseTile();
		}
		Iso.DebugDrawTile(targetTile, Tilemap.instance[targetTile] ? Color.green : Color.red, 0.1f);
		Pathing.BuildPath(iso.tilePos, targetTile, character.directionCount);

		if (Input.GetMouseButton(0)) {
			if (Usable.hot != null) {
				character.Use(Usable.hot);
			} else {
				character.GoTo(targetTile);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			character.Teleport(Iso.MouseTile());
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			foreach (Character character in GameObject.FindObjectsOfType<Character>()) {
				if (this.character != character) {
					SetCharacter(character);
					return;
				}
			}
		}
	}
}