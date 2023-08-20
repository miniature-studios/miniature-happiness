﻿using Common;
using Level.Room;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TileBuilder
{
    public enum GameMode
    {
        God,
        Build,
        Play
    }

    [AddComponentMenu("Scripts/TileBuilder.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private List<CoreModel> coreModels;

        private Validator.IValidator validator = new Validator.GameMode();

        public UnityEvent BuiltValidatedOffice;

        public Result Execute(Command.ICommand command)
        {
            Result response = validator.ValidateCommand(command);
            return response.Success ? command.Execute(tileBuilder) : response;
        }

        public void ChangeGameMode(GameMode game_mode)
        {
            validator = game_mode switch
            {
                GameMode.God => new Validator.GodMode(),
                GameMode.Build => new Validator.BuildMode(tileBuilder),
                GameMode.Play => new Validator.GameMode(),
                _ => throw new ArgumentException(),
            };
        }

        public void ValidateBuilding()
        {
            Result result = Execute(new Command.ValidateBuilding());
            if (result.Success)
            {
                BuiltValidatedOffice?.Invoke();
            }
        }

        public void Hover(CoreModel coreModel)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                coreModel.ModifyPlacingProperties(RotationDirection.Clockwise);
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Command.ShowRoomIllusion command =
                new(
                    coreModel,
                    ray,
                    coreModel.PlacingProperties.PlacingRotation,
                    tileBuilder.BuilderMatrix
                );
            _ = Execute(command);
        }

        public Result Drop(CoreModel coreModel)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Command.DropRoom command = new(coreModel, ray, tileBuilder.BuilderMatrix);
            Result result = Execute(command);
            if (result.Success)
            {
                coreModels.Add(coreModel);
            }
            return result;
        }

        public Result<CoreModel> Borrow()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CoreModel coreModel = null;
            Result result = Execute(
                new Command.BorrowRoom(ray, (borrowedCoreModel) => coreModel = borrowedCoreModel)
            );
            if (result.Success)
            {
                _ = coreModels.Remove(coreModel);
                return new SuccessResult<CoreModel>(coreModel);
            }
            else
            {
                return new FailResult<CoreModel>("Cannot borrow");
            }
        }
    }
}
